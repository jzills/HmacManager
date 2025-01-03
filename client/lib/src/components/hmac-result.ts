import Hmac from "./hmac";

/**
 * Represents the result of an HMAC signing operation.
 */
type HmacResult = {
    /** 
     * The generated HMAC details, or null if the signing operation failed.
     */
    hmac: Hmac | null;

    /** 
     * Indicates whether the signing operation was successful.
     */
    isSuccess: boolean;

    /** 
     * The date and time when the HMAC was generated.
     */
    dateGenerated: Date;
};

export default HmacResult;