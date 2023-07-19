rootDir="$(cd -P "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"

prompt_for_env_var() {
  local key=$1
  local prompt=$2
  local default_value=$3

  if [ -n "$default_value" ]; then
    read -p "$prompt [$default_value]: " var_value
    var_value=${var_value:-$default_value}
  else
    read -p "$prompt: " var_value
  fi

  echo "$key=$var_value"
}

envPath="$rootDir/.env"
if [ ! -f "$envPath" ]; then
  touch "$envPath"

  echo "$(prompt_for_env_var "SSL_PFX_PATH" "SSL PFX file path?" "$rootDir/.ssl/localhost.pfx")" >> "$envPath"

  echo "$(prompt_for_env_var "SSL_PFX_PASSWORD" "SSL PFX file password?")" >> "$envPath"
fi
