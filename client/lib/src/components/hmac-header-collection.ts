import { HmacAuthenticationDefaults } from "../hmac-authentication-defaults";

/**
 * Represents a collection of HMAC headers where each header key is optional 
 * and maps to a value that can be a string or `null`.
 */
type HmacHeaderCollection = { 
    [key in HmacAuthenticationDefaults.Headers]?: string | null;
};

export default HmacHeaderCollection;