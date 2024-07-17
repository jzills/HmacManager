export class SigningContentBuilder {
    private readonly content: any[] = [];

    withPublicKey = (publicKey: string) => {
        this.content.push(publicKey);
        return this;
    }

    withMethod = (method: string) => {
        this.content.push(method);
        return this;
    }

    withPathAndQuery = (pathAndQuery: string) => {
        this.content.push(pathAndQuery);
        return this;
    }

    withAuthority = (authority: string) => {
        this.content.push(authority);
        return this;
    }

    withHost = (host: string) => {
        this.content.push(host);
        return this;
    }

    withRequested = (requestedOn: Date) => {
        const ticks = ((requestedOn.getTime() * 10000) + 621355968000000000);
        this.content.push(ticks);
        return this;
    }

    withBody = (body: ReadableStream<Uint8Array> | null) => {
        const computeContentHash = content => {
            // TODO
            return content;
        }

        this.content.push(computeContentHash(body));
        return this;
    }

    withNonce = (nonce: string) => {
        this.content.push(nonce);
        return this;
    }

    withSignedHeaders = (signedHeaders: string[] = [], headers: Headers) => {
        if (signedHeaders.length) {
            const headerValues = signedHeaders.map(key => headers.get(key));
            const isMissingHeaderValues = headerValues.some(value => value === null);
            if (isMissingHeaderValues) {
                throw new Error("There are missing headers that are required for signing.");
            } else {
                this.content.push(...headerValues);
            }
        }

        return this;
    }

    build = () => this.content.filter(element => element).join(":");  
}