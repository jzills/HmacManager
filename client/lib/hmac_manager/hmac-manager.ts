import { HmacResult } from "./components/hmac-result.js"
import { HmacProvider } from "./components/hmac-provider.js"
import { Hmac } from "./components/hmac.js"
import { HmacPolicy } from "./components/hmac-policy.js"
import { HmacScheme } from "./components/hmac-scheme.js"

export class HmacManager {
    private readonly policy: HmacPolicy;
    private readonly scheme: HmacScheme | null;
    private readonly provider: HmacProvider;

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
        )
    }

    sign = async (request: Request): Promise<HmacResult> => {
        try {
            const dateRequested = new Date();
            const nonce = crypto.randomUUID();

            const { signingContent, signature } = await
                this.provider.compute(request,
                    dateRequested,
                    nonce
                );
            
            const hmac = {
                dateRequested,
                nonce,
                signingContent,
                signature,
                signedHeaders: this.scheme?.headers ?? null
            };
            
            this.addRequiredHeaders(request.headers, hmac);

            return {
                isSuccess: true,
                hmac
            }
        } catch (error: unknown) {
            return {
                isSuccess: false,
                message: (error as Error)?.message
            }
        }
    }

    private addRequiredHeaders(headers: Headers, hmac: Hmac) {
        headers.append("X-Hmac-Policy", `${this.policy.name}`);
        
        if (this.scheme?.name) {
            // TODO: Error if headers are not present from the scheme
            headers.append("X-Hmac-Scheme", `${this.scheme.name}`);
        }

        headers.append("X-Hmac-Date-Requested", `${hmac.dateRequested.getTime()}`);
        headers.append("X-Hmac-Nonce", `${hmac.nonce}`);
        headers.append("Authorization", `Hmac ${hmac.signature}`);
    }
}