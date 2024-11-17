import HmacHeaderBuilder from "./hmac-header-builder.js";
import HmacOptionsHeaderBuilder from "./hmac-options-header-builder.js";

/**
 * Factory class to create instances of HMAC header builders.
 * Depending on the configuration, it creates either a standard HMAC header builder 
 * or a consolidated options header builder.
 */
export default class HmacHeaderBuilderFactory {
    /**
     * Indicates whether the consolidated headers mode is enabled.
     */
    private readonly isConsolidatedHeadersEnabled: boolean;

    /**
     * Initializes a new instance of the HmacHeaderBuilderFactory class.
     * 
     * @param isConsolidatedHeadersEnabled - A boolean value indicating whether 
     * consolidated headers are enabled. If true, the factory produces 
     * HmacOptionsHeaderBuilder instances; otherwise, it produces HmacHeaderBuilder instances.
     */
    constructor(isConsolidatedHeadersEnabled: boolean) {
        this.isConsolidatedHeadersEnabled = isConsolidatedHeadersEnabled;
    }

    /**
     * Creates a new instance of an HMAC header builder.
     * 
     * @returns An instance of either HmacHeaderBuilder or HmacOptionsHeaderBuilder, 
     * depending on whether consolidated headers are enabled.
     */
    create = (): HmacHeaderBuilder =>
        this.isConsolidatedHeadersEnabled ?
            new HmacOptionsHeaderBuilder() :
            new HmacHeaderBuilder();
}