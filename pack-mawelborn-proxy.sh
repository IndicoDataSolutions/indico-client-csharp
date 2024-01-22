podman build -f Dockerfile -t mawelborn-proxy
podman create --name mawelborn-proxy-container mawelborn-proxy
podman cp mawelborn-proxy-container:/indico-client-csharp/IndicoV2/bin/Release/IndicoClient.6.0.0-mawelborn-proxy-f33a3974ea68894c33a3fa515b36867d2485a18f.nupkg .
podman rm mawelborn-proxy-container
