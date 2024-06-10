import { SignatureBuilder } from "../builders/signature-builder.js"
import { SigningContentBuilder } from "../builders/signing-content-builder.js"

export class HmacProvider {
    readonly publicKey: string;
    readonly privateKey: string;
    readonly signedHeaders: string[];

    constructor(publicKey, privateKey, signedHeaders) {
        this.publicKey = publicKey
        this.privateKey = privateKey
        this.signedHeaders = signedHeaders
    }

    computeSigningContent = (request: Request, dateRequested: Date, nonce: string): string => {

        const signingContent = new SigningContentBuilder(request)
            .withPublicKey(this.publicKey)
            .withDateRequested(dateRequested)
            .withNonce(nonce)
            .withHeaderValues(this.signedHeaders)
            //.withContentHash() // TODO
            .build()

        return signingContent
    }

    computeSignature = async (signingContent: string): Promise<string> => {

        const signaturePromise = new SignatureBuilder()
            .withPrivateKey(this.privateKey)
            .withSigningContent(signingContent)
            .build()

        return signaturePromise
    }
}