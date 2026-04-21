#!/usr/bin/env bash
set -euo pipefail
LCOV_FILE="${1:-./coverage/lcov.info}"
if [[ ! -f "$LCOV_FILE" ]]; then
  echo "No lcov file at $LCOV_FILE"
  exit 1
fi
awk '
/^SF:/ { path = substr($0, 4); block = $0 "\n"; next }
/^DA:/ { next }
{ block = block $0 "\n" }
/^end_of_record$/ {
  if (path !~ /\\Generated\\|\\obj\\|\\bin\\|\/Generated\/|\/obj\/|\/bin\//) print block
  block = ""; path = ""
}
' "$LCOV_FILE" > "${LCOV_FILE}.tmp" && mv "${LCOV_FILE}.tmp" "$LCOV_FILE"
