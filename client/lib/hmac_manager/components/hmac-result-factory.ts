import { HmacResult } from "./hmac-result.js";
import { Hmac } from "./hmac.js";

/**
 * Factory class for creating HMAC result instances.
 */
export class HmacResultFactory {
    /**
     * Creates a successful HMAC result.
     * @param hmac - The HMAC details to include in the result.
     * @returns An HMAC result indicating success.
     */
    success = (hmac: Hmac) => this.create(true, hmac);

    /**
     * Creates a failed HMAC result.
     * @returns An HMAC result indicating failure.
     */
    failure = () => this.create(false);

    /**
     * Creates an HMAC result instance.
     * @param isSuccess - Indicates whether the result is successful.
     * @param hmac - The HMAC details, or null if the result is a failure.
     * @returns An HMAC result object.
     */
    private create = (isSuccess: boolean, hmac: Hmac | null = null): HmacResult => ({
        hmac,
        isSuccess,
        dateGenerated: new Date()
    });
}