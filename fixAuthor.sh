#!/bin/bash

echo Reference Email:
read authorEmail
echo New Email:
read authorEmailNew
echo New Name:
read authorNameNew

git filter-branch --env-filter 'if [ "$GIT_AUTHOR_EMAIL" = "$authorEmail" ]; then
     GIT_AUTHOR_EMAIL=$authorEmailNew;
     GIT_AUTHOR_NAME="$authorNameNew";
     GIT_COMMITTER_EMAIL=$GIT_AUTHOR_EMAIL;
     GIT_COMMITTER_NAME="$GIT_AUTHOR_NAME"; fi' -- --all
