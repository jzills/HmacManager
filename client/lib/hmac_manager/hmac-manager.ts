import { HmacResult } from "./components/hmac-result.js"
import { HmacProvider } from "./components/hmac-provider.js"
import { Hmac } from "./components/hmac.js"
import { HmacPolicy } from "./components/hmac-policy.js"
import { HmacScheme } from "./components/hmac-scheme.js"
import { HmacResultFactory } from "./components/hmac-result-factory.js"

export class HmacManager {
    private readonly policy: HmacPolicy;
    private readonly scheme: HmacScheme | null;
    private readonly provider: HmacProvider;
    private readonly resultFactory: HmacResultFactory;

    constructor(
        policy: HmacPolicy,
        scheme: HmacScheme | null = null
    ) {
        this.policy = policy;
        this.scheme = scheme;
        this.provider = new HmacProvider(
            this.policy.publicKey,
            this.policy.privateKey,
            this.scheme?.headers
        );

        this.resultFactory = new HmacResultFactory(
            this.policy.name,
            this.scheme?.name ?? null
        );
    }

    sign = async (request: Request): Promise<HmacResult> => {
        try {
            const {
                dateRequested,
                nonce,
                signingContent,
                signature
            } = await this.computeSignature(request);

            const hmac = {
                dateRequested,
                nonce,
                signingContent,
                signature,
                signedHeaders: this.scheme?.headers ?? null
            };
            
            this.addRequiredHeaders(request.headers, hmac);

            return this.resultFactory.success(hmac);
        } catch (error: unknown) {
            return this.resultFactory.failure();
        }
    }

    private computeSignature = async (request: Request) => {
        const dateRequested = new Date();
        const nonce = crypto.randomUUID();
        const { signingContent, signature } = await this.provider.compute(request,
            dateRequested,
            nonce
        );

        return { dateRequested, nonce, signingContent, signature };
    }

    private addRequiredHeaders(headers: Headers, hmac: Hmac): void {
        headers.append("Hmac-Policy", `${this.policy.name}`);
        
        if (this.scheme?.name) {
            // TODO: Error if headers are not present from the scheme
            headers.append("Hmac-Scheme", `${this.scheme.name}`);
        }

        headers.append("Hmac-Date-Requested", `${hmac.dateRequested.getTime()}`);
        headers.append("Hmac-Nonce", `${hmac.nonce}`);
        headers.append("Authorization", `Hmac ${hmac.signature}`);
    }
}