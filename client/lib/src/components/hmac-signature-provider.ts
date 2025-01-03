import SignatureBuilder from "../builders/signature-builder"
import SigningContentBuilder from "../builders/signing-content-builder"
import HashAlgorithm from "../hash-algorithm";
import HmacSignature from "./hmac-signature";
import { computeContentHash } from "../utilities/hmac-utilities";

/**
 * Provides functionality to create an HMAC signature for request authentication.
 */
export default class HmacSignatureProvider {
    /** 
     * The public key used in the HMAC signing process.
     */
    private readonly publicKey: string;

    /** 
     * The private key used for signing the content in the HMAC process.
     */
    private readonly privateKey: string;

    /** 
     * A list of headers that are included in the HMAC signature.
     * Each string in the array represents a header that has been included.
     */
    private readonly signedHeaders: string[] = [];

    /** 
     * The hashing algorithm used to compute the content hash. 
     * Default is SHA-256.
     */
    private readonly contentHashAlgorithm: HashAlgorithm = HashAlgorithm.SHA256;

    /** 
     * The hashing algorithm used to compute the signature hash. 
     * Default is SHA-256.
     */
    private readonly signatureHashAlgorithm: HashAlgorithm = HashAlgorithm.SHA256;

    private readonly signingContentBuilder: SigningContentBuilder;

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
        contentHashAlgorithm: HashAlgorithm = HashAlgorithm.SHA256,
        signatureHashAlgorithm: HashAlgorithm = HashAlgorithm.SHA256,
        signingContentBuilder: SigningContentBuilder = new SigningContentBuilder()
    ) {
        this.publicKey = publicKey;
        this.privateKey = privateKey;
        this.signedHeaders = signedHeaders;
        this.contentHashAlgorithm = contentHashAlgorithm;
        this.signatureHashAlgorithm = signatureHashAlgorithm;
        this.signingContentBuilder = signingContentBuilder;
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
        const { body } = request.clone();
        const contentHash = await computeContentHash(body, this.contentHashAlgorithm);
        const signingContentBuilder = this.signingContentBuilder.createBuilder()
            .withRequest(request)
            .withPublicKey(this.publicKey)
            .withDateRequested(dateRequested)
            .withContentHash(contentHash)
            .withSignedHeaders(this.signedHeaders)
            .withNonce(nonce);

        const signingContent = await signingContentBuilder.build();
        const signatureBuilder = new SignatureBuilder(this.privateKey,
            signingContent,
            this.signatureHashAlgorithm
        );

        return {
            signingContent,
            signature: await signatureBuilder.build()
        };
    }
}