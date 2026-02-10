set PROXY_INSTALL_DIR=C:\Program Files\mitmproxy\bin
set LISTEN_PORT=8080
set FORWARD_URL=https://possystem-api-sandbox.fiskaltrust.eu

@echo off
echo Starting mitmproxy reverse proxy...
echo Listening on: http://localhost:%LISTEN_PORT%
echo Forwarding to: %FORWARD_URL%
echo Using mitmproxy from: %PROXY_INSTALL_DIR%
echo.
echo Press Ctrl+C to stop the proxy
echo.

"%PROXY_INSTALL_DIR%\mitmproxy.exe" -k -m reverse:%FORWARD_URL%@%LISTEN_PORT%