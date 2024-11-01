import { SignatureBuilder } from "../builders/signature-builder.js"
import { SigningContentBuilder } from "../builders/signing-content-builder.js"
import { computeContentHash } from "../utilities/hmac-utilities.js";
import { HmacSignature } from "./hmac-signature.js";

/**
 * Provides functionality to create an HMAC signature for request authentication.
 */
export class HmacSignatureProvider {
    private readonly publicKey: string;
    private readonly privateKey: string;
    private readonly signedHeaders: string[] = [];
    private readonly contentHashAlgorithm: string = "sha-256";
    private readonly signatureHashAlgorithm: string = "sha-256";

    /**
     * Initializes a new instance of HmacSignatureProvider.
     * @param publicKey - The public key used for signature generation.
     * @param privateKey - The private key used for signature generation.
     * @param signedHeaders - An array of headers to be signed. Defaults to an empty array.
     * @param contentHashAlgorithm - The algorithm used for content hashing. Defaults to "sha-256".
     * @param signatureHashAlgorithm - The algorithm used for signature hashing. Defaults to "sha-256".
     */
    constructor(
        publicKey: string,
        privateKey: string,
        signedHeaders: string[] = [],
        contentHashAlgorithm: string = "sha-256",
        signatureHashAlgorithm: string = "sha-256"
    ) {
        this.publicKey = publicKey;
        this.privateKey = privateKey;
        this.signedHeaders = signedHeaders;
        this.contentHashAlgorithm = contentHashAlgorithm;
        this.signatureHashAlgorithm = signatureHashAlgorithm;
    }

    /**
     * Computes the HMAC signature for a given request, using the specified date and nonce.
     * @param request - The request to be signed.
     * @param dateRequested - The timestamp of the request.
     * @param nonce - A unique identifier for the request.
     * @returns An object containing the signing content and generated signature.
     */
    compute = async (request: Request, dateRequested: Date, nonce: string): Promise<HmacSignature> => {
        // Clone the request since we need to read the body if it exists
        const { method, body, headers, url } = request.clone();
        const { search, host, pathname } = new URL(url);

        const contentHash = await computeContentHash(body, this.contentHashAlgorithm);

        const signingContent = new SigningContentBuilder()
            .withMethod(method)
            .withPathAndQuery(`${pathname}${search}`)
            .withDateRequested(dateRequested)
            .withPublicKey(this.publicKey)
            .withContentHash(contentHash)
            .withSignedHeaders(this.signedHeaders, headers)
            .withNonce(nonce)
            .build();

        const signatureBuilder = new SignatureBuilder(
            this.privateKey,
            signingContent,
            this.signatureHashAlgorithm
        );

        return {
            signingContent,
            signature: await signatureBuilder.build()
        };
    }
}