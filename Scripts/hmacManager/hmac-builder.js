export class HmacBuilder {
    #builder = [];

    withClient = clientId => {
        this.#builder.push(clientId);
        return this;
    }

    withMethod = method => {
        this.#builder.push(method);
        return this;
    }

    withPathAndQuery = pathAndQuery => {
        this.#builder.push(pathAndQuery);
        return this;
    }

    withAuthority = authority => {
        this.#builder.push(authority);
        return this;
    }

    withHost = host => {
        this.#builder.push(host);
        return this;
    }

    withRequested = requestedOn => {
        this.#builder.push(requestedOn);
        return this;
    }

    withBody = body => {
        this.#builder.push(body);
        return this;
    }

    withNonce = nonce => {
        this.#builder.push(nonce);
        return this;
    }

    withSignedHeaders = (signedHeaders, headers) => {
        if (signedHeaders.length) {
            const headerValues = signedHeaders.map(key => headers.get(key));
            if (headerValues.some(header => header === null)) {
                throw new Error("Missing required headers");
            } else {
                this.#builder.push(...headerValues);
            }
        }

        return this;
    }

    build = () => this.#builder.filter(element => element).join(":");
}