export class SigningContentBuilder {
    request: Request;
    builder: string[] = [];
    publicKey: string;
    dateRequested: Date;
    nonce: string;
    headerValues: string[] = [];
    contentHash: string | null;

    constructor(request: Request) {
        this.request = request;
        this.builder.push(request.method);
    }

    withPublicKey = (publicKey: string) => {
        this.publicKey = publicKey;
        return this;
    }

    withDateRequested = (dateRequested: Date) => {
        // TODO: Will need to use getTime and update
        // .NET library to use millisecond time instead
        // of ticks 
        this.dateRequested = dateRequested;
        return this;
    }

    withNonce = (nonce: string) => {
        this.nonce = nonce;
        return this;
    }

    withHeaderValues = (signedHeaders: string[]) => {
        if (signedHeaders?.length) {
            const headerValues = signedHeaders.map(key => this.request.headers.get(key));
            if (headerValues.some(header => header === null)) {
                throw new Error("Missing required headers");
            } else {
                this.headerValues = signedHeaders;
            }
        }

        return this;
    }

    withContentHash = (contentHash: string | null) => {
        this.contentHash = contentHash;
        return this;
    }

    build = () => {
    
        const regex = new RegExp("^(?:[a-z+]+:)?//", "i");

        // Check if url is absolute
        if (regex.test(this.request.url)) {
            const { 
                pathname, 
                search, 
                host 
            } = new URL(this.request.url);

            this.builder.push(`${pathname}${search}`);
            this.builder.push(host);
        } else {
            this.builder.push(this.request.url);
        }

        this.builder.push(`${this.dateRequested}`);
        this.builder.push(`${this.publicKey}`);

        if (this.contentHash) {
            this.builder.push(`${this.contentHash}`);
        }

        if (this.headerValues.length) {
            this.builder.push(...this.headerValues.map(value => `:${value}`));
        } 

        this.builder.push(this.nonce);
        return this.builder.filter(element => element).join(":");
    }
}