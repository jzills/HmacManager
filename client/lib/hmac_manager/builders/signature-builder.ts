import { getByteArray, getKeyBytes, getSignature, getUnicodeForm } from "../utilities/hmac-utilities.js";
import { Algorithm } from "../components/algorithm.js";

/**
 * Class responsible for building a signature using HMAC.
 */
export class SignatureBuilder {
    private readonly privateKey: string;
    private readonly signingContent: string;
    private readonly algorithm: Algorithm;

    /**
     * Initializes a new instance of the SignatureBuilder.
     * @param privateKey The private key used for signing.
     * @param signingContent The content to sign.
     * @param signatureHashAlgorithm The hash algorithm to use (default is "sha-256").
     */
    constructor(
        privateKey: string,
        signingContent: string,
        signatureHashAlgorithm: string = "sha-256"
    ) {
        this.privateKey = privateKey;
        this.signingContent = signingContent;
        this.algorithm = { name: "HMAC", hash: signatureHashAlgorithm };
    }

    /**
     * Builds the HMAC signature asynchronously.
     * @returns A promise that resolves to the base64-encoded signature.
     */
    build = async () => {
        const signatureBytes = await getSignature(
            await getKeyBytes(this.privateKey, this.algorithm),
            getByteArray(this.signingContent)
        );

        const unicodeForm = getUnicodeForm(signatureBytes);
        return btoa(unicodeForm); // Convert to base64
    }
}