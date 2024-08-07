import { getByteArray, getUnicodeForm } from "../utilities/hmac-utilities.js";

export type Algorithm = { name: string, hash: string };

export class SignatureBuilder {
    private readonly privateKey: string
    private readonly signingContent: string
    private readonly algorithm: Algorithm

    constructor(
        privateKey: string,
        signingContent: string,
        signatureHashAlgorithm: string = "sha-256"
    ) {
        this.privateKey = privateKey
        this.signingContent = signingContent
        this.algorithm = { name: "HMAC", hash: signatureHashAlgorithm }
    }

    build = async () => {

        const assertValidBuild = () => {
            const isMissingRequiredValues =
                this.privateKey === null ||
                this.signingContent === null

            if (isMissingRequiredValues) {
                throw new Error("Required values are missing.")
            }
        }

        assertValidBuild();

        const getKeyBytes = async () => crypto.subtle.importKey("raw",
            getByteArray(atob(this.privateKey)),
            this.algorithm,
            false,
            ["sign"]
        )

        const getSignature = async (keyBytes: CryptoKey, signingContentBytes: BufferSource) =>
            crypto.subtle.sign("HMAC", keyBytes, signingContentBytes)

        const signatureBytes = await getSignature(await
            getKeyBytes(),
            getByteArray(this.signingContent)
        )

        const unicodeForm = getUnicodeForm(signatureBytes)
        return btoa(unicodeForm)
    }
}