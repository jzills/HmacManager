export class SignatureBuilder {
    privateKey = null;
    signingContent = null;
    algorithm = { name: "HMAC", hash: "SHA-256" };
    withPrivateKey(privateKey) {
        this.privateKey = privateKey;
        return this;
    }
    withSigningContent(signingContent) {
        this.signingContent = signingContent;
        return this;
    }
    build = async () => {
        const assertValidBuild = () => {
            const isMissingRequiredValues = this.privateKey === null ||
                this.signingContent === null;
            if (isMissingRequiredValues) {
                throw new Error("Required values are missing.");
            }
        };
        assertValidBuild();
        const getByteArray = (content) => Uint8Array.from(content, element => element.toString().charCodeAt(0));
        const getUnicodeForm = (signatureBytes) => {
            const bytes = new Uint8Array(signatureBytes);
            const bytesSplit = bytes.toString().split(",");
            const unicodeForm = bytesSplit.map(element => String.fromCharCode(parseInt(element))).join("");
            return unicodeForm;
        };
        const getKeyBytes = async () => crypto.subtle.importKey("raw", getByteArray(atob(this.privateKey)), this.algorithm, false, ["sign"]);
        const getSignature = async (keyBytes, signingContentBytes) => crypto.subtle.sign("HMAC", keyBytes, signingContentBytes);
        const signatureBytes = await getSignature(await getKeyBytes(), getByteArray(this.signingContent));
        const unicodeForm = getUnicodeForm(signatureBytes);
        return btoa(unicodeForm);
    };
}
