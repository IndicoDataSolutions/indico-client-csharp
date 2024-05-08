podman build -f Dockerfile -t mawelborn-proxy
podman create --name mawelborn-proxy-container mawelborn-proxy
podman cp mawelborn-proxy-container:/indico-client-csharp/IndicoV2/bin/Release/IndicoClient.6.6.0-mawelborn-proxy-e49f9dc863434d37f9f3dd366a4bb55050bb3e02.nupkg .
podman rm mawelborn-proxy-container
