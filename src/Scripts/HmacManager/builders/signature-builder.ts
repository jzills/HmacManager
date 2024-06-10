import type { webcrypto } from "crypto";

export class SignatureBuilder {
    privateKey: string | null = null;
    signingContent: string | null = null;

    algorithm = { name: "HMAC", hash: "SHA-256" };

    withPrivateKey(privateKey: string) {
        this.privateKey = privateKey
        return this
    }

    withSigningContent(signingContent: string) {
        this.signingContent = signingContent
        return this
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

        const getByteArray = (content: string) => 
            Uint8Array.from(content, 
                element => element.toString().charCodeAt(0))

        const getUnicodeForm = (signatureBytes: ArrayBuffer) => {
            const bytes = new Uint8Array(signatureBytes)
            const bytesSplit = bytes.toString().split(",")
            const unicodeForm = bytesSplit.map(element => String.fromCharCode(parseInt(element))).join("")
            return unicodeForm
        }

        const getKeyBytes = async () => crypto.subtle.importKey("raw", 
            getByteArray(atob(this.privateKey as string)), 
            this.algorithm, 
            false, 
            ["sign"]
        )

        const getSignature = async (keyBytes: webcrypto.CryptoKey, signingContentBytes: webcrypto.BufferSource) => 
            crypto.subtle.sign("HMAC", keyBytes, signingContentBytes)

        const signatureBytes = await getSignature(await 
            getKeyBytes(), 
            getByteArray(this.signingContent as string)
        )

        const unicodeForm = getUnicodeForm(signatureBytes)
        return btoa(unicodeForm)
    }
}