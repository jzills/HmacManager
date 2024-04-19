import { SignatureBuilder } from "../builders/signature-builder.js"
import { SigningContentBuilder } from "../builders/signing-content-builder.js"

export class HmacProvider {
    constructor(publicKey, privateKey, signedHeaders) {
        this.publicKey = publicKey
        this.privateKey = privateKey
        this.signedHeaders = signedHeaders
    }

    computeSigningContent = (request, dateRequested, nonce) => {

        const { 
            headers, 
            method, 
            body, 
            url 
        } = request 

        const { 
            pathname, 
            search, 
            host 
        } = new URL(url)
    
        const signingContent = new SigningContentBuilder()
            .withPublicKey(this.publicKey)
            .withMethod(method)
            .withPathAndQuery(`${pathname}${search}`)
            .withAuthority(host)
            .withRequested(dateRequested)
            .withBody(body)
            .withNonce(nonce)
            .withSignedHeaders(this.signedHeaders, headers)
            .build()

        return signingContent
    }

    computeSignature = async signingContent => {

        const signaturePromise = new SignatureBuilder()
            .withPrivateKey(this.privateKey)
            .withSigningContent(signingContent)
            .build()

        return await signaturePromise
    }
}