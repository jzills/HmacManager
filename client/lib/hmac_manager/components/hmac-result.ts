import { Hmac } from "./hmac.js";

/**
 * Represents the result of an HMAC signing operation.
 */
export type HmacResult = {
    /** 
     * The policy used for generating the HMAC.
     */
    policy: string;

    /** 
     * The scheme used for generating the HMAC, or null if not applicable.
     */
    scheme: string | null;

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