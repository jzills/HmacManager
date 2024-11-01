/**
 * Builder for constructing the signing content used in HMAC authentication.
 */
export class SigningContentBuilder {
    private readonly content: any[] = [];

    /** 
     * Adds the public key to the signing content.
     * @param publicKey The public key to include.
     * @returns The instance of the builder for method chaining.
     */
    withPublicKey = (publicKey: string) => {
        this.content.push(publicKey);
        return this;
    }

    /** 
     * Adds the HTTP method (e.g., GET, POST) to the signing content.
     * @param method The HTTP method to include.
     * @returns The instance of the builder for method chaining.
     */
    withMethod = (method: string) => {
        this.content.push(method);
        return this;
    }

    /** 
     * Adds the path and query string to the signing content.
     * @param pathAndQuery The path and query string to include.
     * @returns The instance of the builder for method chaining.
     */
    withPathAndQuery = (pathAndQuery: string) => {
        this.content.push(pathAndQuery);
        return this;
    }

    /** 
     * (Pending solution) Intended to add the authority (e.g., host) to the signing content.
     * Currently, this is not implemented, and only the path and query will be added.
     * @param authority The authority to include.
     * @returns The instance of the builder for method chaining.
     */
    withAuthority = (authority: string) => {
        // Pending solution in .NET code. For now, the
        // pathAndQuery will be the only URL component added
        // to the signing content.
        // this.content.push(authority);
        return this;
    }

    /** 
     * (Pending solution) Intended to add the host to the signing content.
     * Currently, this is not implemented, and only the path and query will be added.
     * @param host The host to include.
     * @returns The instance of the builder for method chaining.
     */
    withHost = (host: string) => {
        // Pending solution in .NET code. For now, the
        // pathAndQuery will be the only URL component added
        // to the signing content.
        // this.content.push(host);
        return this;
    }

    /** 
     * Adds the date when the request was made to the signing content.
     * @param requestedOn The date of the request.
     * @returns The instance of the builder for method chaining.
     */
    withDateRequested = (requestedOn: Date) => {
        this.content.push(requestedOn.getTime());
        return this;
    }

    /** 
     * Adds the content hash to the signing content.
     * @param hash The content hash to include.
     * @returns The instance of the builder for method chaining.
     */
    withContentHash = (hash: string | null) => {
        this.content.push(hash);
        return this;
    }

    /** 
     * Adds a nonce (unique identifier) to the signing content.
     * @param nonce The nonce to include.
     * @returns The instance of the builder for method chaining.
     */
    withNonce = (nonce: string) => {
        this.content.push(nonce);
        return this;
    }

    /** 
     * Adds signed headers to the signing content.
     * Throws an error if any required headers are missing.
     * @param signedHeaders The headers that are signed.
     * @param headers The Headers object to extract header values from.
     * @returns The instance of the builder for method chaining.
     */
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

    /** 
     * Builds the signing content by joining the individual components.
     * @returns The constructed signing content as a string.
     */
    build = () => this.content.filter(element => element).join(":");
}