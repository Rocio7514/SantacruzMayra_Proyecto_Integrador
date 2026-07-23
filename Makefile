DOTNET ?= dotnet
HOST ?= 0.0.0.0
PORT ?= 5001

.PHONY: restore build run

restore:
	$(DOTNET) restore

build:
	$(DOTNET) build --no-restore

run:
	$(DOTNET) run --no-launch-profile --urls "http://$(HOST):$(PORT)"
