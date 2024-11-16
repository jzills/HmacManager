import { HmacAuthenticationDefaults } from "../hmac-authentication-defaults.js";
import HmacHeaderBuilder from "./hmac-header-builder.js";

/**
 * A subclass of `HmacHeaderBuilder` that adds functionality for constructing
 * the HMAC authentication headers specifically for the options header.
 * This builder formats a collection of headers into a query string, base64 encodes it,
 * and combines it with the `Authorization` header value to return a complete HMAC header set.
 * 
 * Example usage:
 * ```typescript
 * const builder = new HmacOptionsHeaderBuilder();
 * const headers = builder
 *     .withAuthorization('signature123')
 *     .withPolicy('policyABC')
 *     .withScheme('HMAC')
 *     .withNonce('nonce123')
 *     .withDateRequested('2024-11-15')
 *     .build();
 * ```
 */
export type HmacHeaderCollection = { 
    [key in HmacAuthenticationDefaults.Headers]?: string | null;
};

export default class HmacOptionsHeaderBuilder extends HmacHeaderBuilder {

    /**
     * Builds the HMAC header collection specifically for options, which includes the `Options`
     * header and the `Authorization` header. The `Options` header is constructed by formatting
     * all non-null header values as a query string, then base64 encoding that string.
     * 
     * @returns A collection of headers with the base64 encoded `Options` header and the `Authorization` header.
     */
    build = (): HmacHeaderCollection => { 
        const headerOptionsValueFormatted = Object.entries(this.NonEmptyHeaderValues)
            .map(this.formatAsEquality)  // Formats each entry as `key=value`
            .join("&");  // Joins all entries with "&"
        
        const headerOptionsValue = btoa(headerOptionsValueFormatted);  // Base64 encodes the query string
        return {
            [HmacAuthenticationDefaults.Headers.Options]: headerOptionsValue,
            [HmacAuthenticationDefaults.Headers.Authorization]: this.HeaderValues[HmacAuthenticationDefaults.Headers.Authorization]
        };
    }

    /**
     * Formats a single header entry as a key-value pair in the form of `key=value`.
     * 
     * @param entry A tuple representing a header key and its associated value.
     * @returns A formatted string in the form of `key=value`.
     */
    private formatAsEquality = ([key, value]: [key: string, value: string | null]) => `${key}=${value}`;
}