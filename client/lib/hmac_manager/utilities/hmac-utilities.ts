/**
 * Converts an ArrayBuffer of bytes to a Unicode string.
 * @param signatureBytes - The ArrayBuffer containing byte data.
 * @returns A string representation in Unicode form.
 */
export const getUnicodeForm = (signatureBytes: ArrayBuffer): string => {
    const bytes = new Uint8Array(signatureBytes);
    const bytesSplit = bytes.toString().split(",");
    const unicode = bytesSplit.map(element => String.fromCharCode(parseInt(element))).join("");
    return unicode;
}

/**
 * Computes a base64-encoded content hash from a given ReadableStream and algorithm.
 * @param body - The ReadableStream containing data to hash.
 * @param algorithm - The hashing algorithm to use.
 * @returns A base64 string of the hash if body exists; otherwise, null.
 */
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

/**
 * Converts a string into a Uint8Array of byte values.
 * @param content - The string to convert.
 * @returns Uint8Array of byte values from the string.
 */
export const getByteArray = (content: string): Uint8Array =>
    Uint8Array.from(content,
        element => element.charCodeAt(0));

/**
 * Converts a base64 private key string to a CryptoKey object for signing.
 * @param privateKey - Base64-encoded private key.
 * @param algorithm - The signing algorithm to use.
 * @returns A CryptoKey object for signing.
 */
export const getKeyBytes = async (privateKey: string, algorithm: Algorithm) =>
    crypto.subtle.importKey("raw",
        getByteArray(atob(privateKey)),
        algorithm,
        false,
        ["sign"]
    );

/**
 * Signs the provided content using a CryptoKey and returns the signature.
 * @param keyBytes - The CryptoKey used for signing.
 * @param signingContentBytes - The data to be signed.
 * @returns A Promise that resolves with the signature as an ArrayBuffer.
 */
export const getSignature = async (keyBytes: CryptoKey, signingContentBytes: BufferSource) =>
    crypto.subtle.sign("HMAC", keyBytes, signingContentBytes);