podman build -f Dockerfile -t mawelborn-proxy
podman create --name mawelborn-proxy-container mawelborn-proxy
podman cp mawelborn-proxy-container:/indico-client-csharp/IndicoV2/bin/Release/IndicoClient.6.6.0-mawelborn-proxy-7df00caf525640900903f5a944730a10a61fd3af.nupkg .
podman rm mawelborn-proxy-container
