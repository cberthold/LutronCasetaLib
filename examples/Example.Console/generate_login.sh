#!/bin/bash

# Usage: get_lutron_cert.sh [bridge_ip] | tee cert.pem

function error() {
  echo "Error: $1" >&2
  exit 1
}

login_server="device-login.lutron.com"
app_client_id="e001a4471eb6152b7b3f35e549905fd8589dfcf57eb680b6fb37f20878c28e5a"
app_client_secret="b07fee362538d6df3b129dc3026a72d27e1005a3d1e5839eed5ed18c63a89b27"
app_oauth_redirect_page="lutron_app_oauth_redirect"
cert_subject="/C=US/ST=Pennsylvania/L=Coopersburg/O=Lutron Electronics Co., Inc./CN=Lutron Caseta App"

base_url="https://${login_server}/"
redirect_uri_param="https%3A%2F%2F${login_server}%2F${app_oauth_redirect_page}"

authorize_url="${base_url}oauth/authorize?client_id=${app_client_id}&redirect_uri=${redirect_uri_param}&response_type=code"

openssl version >/dev/null || error "openssl required"
jq --version >/dev/null || error "jq required"

echo "Open Browser and login at ${authorize_url}" >&2

echo "Enter the URL (of the \"error\" page you got redirected to (or the code in the URL): " >&2
read -r redirected_url

oauth_code=`echo ${redirected_url} | sed -e's/^\(.*\?code=\)\{0,1\}\([0-9a-f]*\)\s*$/\2/' -e 't' -e 'd'`

[ -n "$oauth_code" ] || error "Invalid code"

private_key="`openssl genrsa 2048 2>/dev/null`"

escaped_csr="`echo \"$private_key\" | openssl req -new -key /dev/fd/0 -subj \"${cert_subject}\" | awk 'NF {sub(/\r/, \"\"); printf \"%s\\\\n\",$0;}'`"

[ -n "$escaped_csr" ] || error "Couldn't generate CSR"

token="`curl -s -X POST -d \"code=${oauth_code}&client_id=${app_client_id}&client_secret=${app_client_secret}&redirect_uri=${redirect_uri_param}&grant_type=authorization_code\" ${base_url}oauth/token`"

[ "bearer" == "`echo \"$token\" | jq -r '.token_type'`" ] || error "Received invalid token $token. Try generating a new code (one time use)."

access_token="`echo \"$token\" | jq -r '.access_token'`"

pairing_request_content="{\"remote_signs_app_certificate_signing_request\":\"${escaped_csr}\"}"

pairing_response="`echo \"$pairing_request_content\" | curl -s -X POST -H \"X-DeviceType: Caseta,RA2Select\" -H \"Content-Type: application/json\" -H \"Authorization: Bearer ${access_token}\" -d \"@-\" ${base_url}api/v1/remotepairing/application/user`"

#echo "$pairing_response"

app_cert="`echo \"$pairing_response\" | jq -r '.remote_signs_app_certificate'`"
remote_cert="`echo \"$pairing_response\" | jq -r '.local_signs_remote_certificate'`"

echo "$app_cert" | openssl x509 -noout || error "Received invalid app cert in pairing response $pairing_response"
echo "$remote_cert" | openssl x509 -noout || error "Received invalid remote cert in pairing response $pairing_response"

echo -e "$private_key\n$app_cert\n$remote_cert"

server_addr=$1
[ -n "$server_addr" ] || exit 0

echo "$app_cert" | { echo "$private_key" | { echo "$remote_cert" | {
  leap_response=`(echo '{"CommuniqueType":"ReadRequest","Header":{"Url":"/server/1/status/ping"}}'; sleep 3) 3<&- 4<&- 5<&- | openssl s_client -connect "$server_addr:8081" -cert /dev/fd/3 -key /dev/fd/4 -CAfile /dev/fd/5 -quiet -no_ign_eof 2>/dev/null`
  [ "$?" -eq "0" ] || error "Could not connect to bridge"
  echo "Successfully connected to bridge, running LEAP Server version `echo \"$leap_response\" | jq -r '.Body.PingResponse.LEAPVersion'`" >&2
} 5<&0; } 4<&0; } 3<&0