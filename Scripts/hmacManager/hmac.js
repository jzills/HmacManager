export class Hmac {
    constructor(
        signature, 
        requestedOn, 
        nonce, 
        signingContent, 
        signedHeaders
    ) {
        this.signature = signature;
        this.requestedOn = requestedOn;
        this.nonce = nonce;
        this.signingContent = signingContent;
        this.signedHeaders = signedHeaders;
    }
}