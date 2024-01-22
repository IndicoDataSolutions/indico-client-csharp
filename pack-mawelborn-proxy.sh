podman build -f Dockerfile -t mawelborn-proxy
podman create --name mawelborn-proxy-container mawelborn-proxy
podman cp mawelborn-proxy-container:/indico-client-csharp/IndicoV2/bin/Release/IndicoClient.6.6.0-mawelborn-proxy-8fcdcb7dec51954781d5862424838dd5aad634bd.nupkg .
podman rm mawelborn-proxy-container
