import { SignatureBuilder } from "../builders/signature-builder.js";
import { SigningContentBuilder } from "../builders/signing-content-builder.js";
export class HmacProvider {
    publicKey;
    privateKey;
    signedHeaders;
    constructor(publicKey, privateKey, signedHeaders) {
        this.publicKey = publicKey;
        this.privateKey = privateKey;
        this.signedHeaders = signedHeaders;
    }
    computeSigningContent = (request, dateRequested, nonce) => {
        const signingContent = new SigningContentBuilder(request)
            .withPublicKey(this.publicKey)
            .withDateRequested(dateRequested)
            .withNonce(nonce)
            .withHeaderValues(this.signedHeaders)
            //.withContentHash() // TODO
            .build();
        return signingContent;
    };
    computeSignature = async (signingContent) => {
        const signaturePromise = new SignatureBuilder()
            .withPrivateKey(this.privateKey)
            .withSigningContent(signingContent)
            .build();
        return await signaturePromise;
    };
}
