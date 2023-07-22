import { SignatureBuilder } from "../builders/signature-builder.js"
import { SigningContentBuilder } from "../builders/signing-content-builder.js"

export class HmacProvider {
    constructor(clientId, clientSecret, signedHeaders) {
        this.clientId = clientId
        this.clientSecret = clientSecret
        this.signedHeaders = signedHeaders
    }

    computeSigningContent = (request, requestedOn, nonce) => {

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
            .withClient(this.clientId)
            .withMethod(method)
            .withPathAndQuery(`${pathname}${search}`)
            .withAuthority(host)
            .withRequested(requestedOn)
            .withBody(body)
            .withNonce(nonce)
            .withSignedHeaders(this.signedHeaders, headers)
            .build()

        return signingContent
    }

    computeSignature = async signingContent => {

        const signaturePromise = new SignatureBuilder()
            .withClientSecret(this.clientSecret)
            .withSigningContent(signingContent)
            .build()

        return await signaturePromise
    }
}