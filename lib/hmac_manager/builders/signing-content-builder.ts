export class SigningContentBuilder {
    private readonly content: any[] = []

    withPublicKey = (publicKey: string) => {
        this.content.push(publicKey)
        return this
    }

    withMethod = (method: string) => {
        this.content.push(method)
        return this
    }

    withPathAndQuery = (pathAndQuery: string) => {
        this.content.push(pathAndQuery)
        return this
    }

    withAuthority = (authority: string) => {
        this.content.push(authority)
        return this
    }

    withHost = (host: string) => {
        this.content.push(host)
        return this
    }

    withDateRequested = (requestedOn: Date) => {
        this.content.push(requestedOn.getTime())
        return this
    }

    withContentHash = (hash: string | null) => {
        this.content.push(hash);
        return this;
    }

    withNonce = (nonce: string) => {
        this.content.push(nonce)
        return this
    }

    withSignedHeaders = (signedHeaders: string[] = [], headers: Headers) => {
        if (signedHeaders.length) {
            const headerValues = signedHeaders.map(key => headers.get(key))
            const isMissingHeaderValues = headerValues.some(value => value === null)
            if (isMissingHeaderValues) {
                throw new Error("There are missing headers that are required for signing.")
            } else {
                this.content.push(...headerValues)
            }
        }

        return this
    }

    build = () => this.content.filter(element => element).join(":")
}