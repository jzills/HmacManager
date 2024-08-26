import { getByteArray, getKeyBytes, getSignature, getUnicodeForm } from "../utilities/hmac-utilities.js";
import { Algorithm } from "../components/algorithm.js";

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
        const signatureBytes = await getSignature(await
            getKeyBytes(this.privateKey, this.algorithm),
            getByteArray(this.signingContent)
        )

        const unicodeForm = getUnicodeForm(signatureBytes)
        return btoa(unicodeForm)
    }
}