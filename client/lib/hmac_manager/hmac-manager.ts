import { HmacResult } from "./components/hmac-result.js"
import { HmacSignatureProvider } from "./components/hmac-signature-provider.js"
import { Hmac } from "./components/hmac.js"
import { HmacPolicy } from "./components/hmac-policy.js"
import { HmacScheme } from "./components/hmac-scheme.js"
import { HmacResultFactory } from "./components/hmac-result-factory.js"
import HmacHeaderBuilder from "./builders/hmac-header-builder.js"

/**
 * HmacManager is responsible for handling HMAC signing of requests.
 * Initializes the required policy, scheme, provider, and result factory to generate signed requests.
 */
export class HmacManager {
    /** 
     * The policy configuration for HMAC, containing rules and constraints.
     */
    private readonly policy: HmacPolicy;

    /** 
     * The specific HMAC scheme in use; may be null if no scheme is defined.
     */
    private readonly scheme: HmacScheme | null;

    /** 
     * A builder used to construct headers required for HMAC authentication.
     */
    private readonly headerBuilder: HmacHeaderBuilder;

    /** 
     * Responsible for generating and verifying HMAC signatures.
     */
    private readonly provider: HmacSignatureProvider;

    /** 
     * Produces results from HMAC operations, such as success or failure outcomes.
     */
    private readonly resultFactory: HmacResultFactory;

    /**
     * Constructs an HmacManager instance.
     * @param policy - The policy defining the public and private keys for HMAC signing.
     * @param scheme - The scheme specifying headers required for HMAC (optional).
     */
    constructor(
        policy: HmacPolicy,
        scheme: HmacScheme | null = null,
        headerBuilder: HmacHeaderBuilder
    ) {
        this.policy = policy;
        this.scheme = scheme;
        this.headerBuilder = headerBuilder;
        this.provider = new HmacSignatureProvider(
            this.policy.publicKey,
            this.policy.privateKey,
            this.scheme?.headers ?? []
        );

        this.resultFactory = new HmacResultFactory();
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
                policy: this.policy.name,
                scheme: this.scheme?.name ?? null,
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
        const builder = this.headerBuilder.createBuilder()
            .withAuthorization(hmac.signature)
            .withPolicy(hmac.policy)
            .withScheme(hmac.scheme)
            .withNonce(hmac.nonce)
            .withDateRequested(hmac.dateRequested);
        
        const hmacHeaders = builder.build();
        for (const [name, value] of Object.entries(hmacHeaders)) {
            headers.append(name, value as string);
        }
    }
}