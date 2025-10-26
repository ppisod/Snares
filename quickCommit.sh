#!/bin/bash
TMPFILE=$(mktemp /tmp/git-commit-status-message.XXX)
git add -A
git status --porcelain \
  | grep '^[MARCDT]' \
  | sed -re 's|^[MARCDT ]+.*/([^/]+)$|\1|' \
  | sort \
  | uniq \
  | tr '\n' ' ' \
  > "$TMPFILE"
git commit -F "$TMPFILE"
rm -f "$TMPFILE"
