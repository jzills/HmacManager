/**
 * Represents an HMAC scheme used for signing requests.
 */
type HmacScheme = {
    /** 
     * The name of the HMAC scheme.
     */
    name: string;

    /** 
     * An array of headers that are included in the HMAC signature.
     */
    headers: string[];
};

export default HmacScheme;