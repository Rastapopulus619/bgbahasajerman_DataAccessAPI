#!/bin/bash

# Determine current Git branch
current_branch=$(git rev-parse --abbrev-ref HEAD)

echo ">>> Current branch is: $current_branch"
echo ">>> Pulling latest changes from origin/$current_branch..."
git pull origin "$current_branch"

echo ">>> Rebuilding Docker container..."
docker-compose build

echo ">>> Restarting container..."
docker-compose down
docker-compose up -d

echo ">>> Redeploy of '$current_branch' complete."
