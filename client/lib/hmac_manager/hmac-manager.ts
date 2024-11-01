import { HmacResult } from "./components/hmac-result.js"
import { HmacSignatureProvider } from "./components/hmac-signature-provider.js"
import { Hmac } from "./components/hmac.js"
import { HmacPolicy } from "./components/hmac-policy.js"
import { HmacScheme } from "./components/hmac-scheme.js"
import { HmacResultFactory } from "./components/hmac-result-factory.js"
import { HmacAuthenticationDefaults } from "./hmac-authentication-defaults.js"

/**
 * HmacManager is responsible for handling HMAC signing of requests.
 * Initializes the required policy, scheme, provider, and result factory to generate signed requests.
 */
export class HmacManager {
    private readonly policy: HmacPolicy;
    private readonly scheme: HmacScheme | null;
    private readonly provider: HmacSignatureProvider;
    private readonly resultFactory: HmacResultFactory;

    /**
     * Constructs an HmacManager instance.
     * @param policy - The policy defining the public and private keys for HMAC signing.
     * @param scheme - The scheme specifying headers required for HMAC (optional).
     */
    constructor(
        policy: HmacPolicy,
        scheme: HmacScheme | null = null
    ) {
        this.policy = policy;
        this.scheme = scheme;
        this.provider = new HmacSignatureProvider(
            this.policy.publicKey,
            this.policy.privateKey,
            this.scheme?.headers
        );

        this.resultFactory = new HmacResultFactory(
            this.policy.name,
            this.scheme?.name ?? null
        );
    }

    /**
     * Signs an HTTP request with HMAC headers.
     * @param request - The HTTP request to sign.
     * @returns A Promise that resolves to an HmacResult indicating success or failure.
     */
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

    /**
     * Computes the signature components needed for HMAC authentication.
     * @param request - The HTTP request to compute the signature for.
     * @returns A Promise with dateRequested, nonce, signingContent, and signature.
     */
    private computeSignature = async (request: Request) => {
        const dateRequested = new Date();
        const nonce = crypto.randomUUID();
        const { signingContent, signature } = await this.provider.compute(request,
            dateRequested,
            nonce
        );

        return { dateRequested, nonce, signingContent, signature };
    }

    /**
     * Adds the required HMAC headers to an HTTP request.
     * @param headers - The headers of the HTTP request.
     * @param hmac - The HMAC object containing the signature data.
     */
    private addRequiredHeaders(headers: Headers, hmac: Hmac): void {
        headers.append(HmacAuthenticationDefaults.Headers.Policy, `${this.policy.name}`);
        
        if (this.scheme?.name) {
            // TODO: Error if headers are not present from the scheme
            headers.append(HmacAuthenticationDefaults.Headers.Scheme, `${this.scheme.name}`);
        }

        headers.append(HmacAuthenticationDefaults.Headers.DateRequested, `${hmac.dateRequested.getTime()}`);
        headers.append(HmacAuthenticationDefaults.Headers.Nonce, `${hmac.nonce}`);
        headers.append("Authorization", `${HmacAuthenticationDefaults.AuthenticationScheme} ${hmac.signature}`);
    }
}