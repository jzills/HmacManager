export const getUnicodeForm = (signatureBytes: ArrayBuffer): string => {
    const bytes = new Uint8Array(signatureBytes);
    const bytesSplit = bytes.toString().split(",");
    const unicodeForm = bytesSplit.map(element => String.fromCharCode(parseInt(element))).join("");
    return unicodeForm;
}

export const computeContentHash = async (body: ReadableStream<Uint8Array> | null, algorithm: AlgorithmIdentifier) => {
    if (body) {
        var streamResult = await body.getReader().read();
        if (streamResult.value) {
            const value = await crypto.subtle.digest(algorithm, streamResult.value);
            const unicodeForm = getUnicodeForm(value);
            return btoa(unicodeForm);
        }
    }

    return null;
}

export const getByteArray = (content: string): Uint8Array =>
    Uint8Array.from(content,
        element => element.charCodeAt(0));

export const getKeyBytes = async (privateKey: string, algorithm: Algorithm) =>
    crypto.subtle.importKey("raw",
        getByteArray(atob(privateKey)),
        algorithm,
        false,
        ["sign"]
    );

export const getSignature = async (keyBytes: CryptoKey, signingContentBytes: BufferSource) =>
    crypto.subtle.sign("HMAC", keyBytes, signingContentBytes);