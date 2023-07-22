import { Hmac } from "./hmac.js";
import { HmacResult } from "./hmac-result.js";
import { SigningContentBuilder } from "./signing-content-builder.js";

export class HmacManager {
    constructor(clientId, clientSecret, signedHeaders) {
        this.clientId = clientId;
        this.clientSecret = clientSecret;
        this.signedHeaders = signedHeaders;
        this.algorithm = { 
            name: "HMAC", 
            hash: "SHA-256" 
        };
    }

    sign = async request => {
        const hmac = new Hmac();
        hmac.requestedOn = new Date();
        hmac.nonce = crypto.randomUUID();
        hmac.signingContent = this.computeSigningContent(request, hmac.requestedOn, hmac.nonce);
        hmac.signature = await this.computeSignature(hmac.signingContent);
        return new HmacResult(hmac, true);
    }

    computeSigningContent = (request, requestedOn, nonce) => {
        const { method, url, headers, body } = request; 
        const { pathname, search, host } = new URL(url);

        const signingContent = new SigningContentBuilder()
            .withClient(this.clientId)
            .withMethod(method)
            .withPathAndQuery(`${pathname}${search}`)
            .withAuthority(host)
            .withRequested(requestedOn)
            .withBody(this.computeContentHash(body))
            .withNonce(nonce)
            .withSignedHeaders(this.signedHeaders, headers)
            .build();

        return signingContent;
    }

    computeContentHash = content => {

    }

    computeSignature = async signingContent => {
        const clientSecretBytes = this.getByteArray(atob(this.clientSecret));
        const keyBytes = await crypto.subtle.importKey("raw", clientSecretBytes, this.algorithm, false, ["sign"]);
        const signingContentBytes = this.getByteArray(signingContent);
        const signatureBytes = await crypto.subtle.sign("HMAC", keyBytes, signingContentBytes);
        const unicodeForm = this.getUnicodeForm(signatureBytes);
        return btoa(unicodeForm);
    }

    getByteArray = content => 
        Uint8Array.from(content, 
            element => element.charCodeAt(0));

    getUnicodeForm = signatureBytes => {
        const bytes = new Uint8Array(signatureBytes);
        const bytesSplit = bytes.toString().split(",");
        const unicodeForm = bytesSplit.map(element => String.fromCharCode(element)).join("");
        return unicodeForm;
    }
}