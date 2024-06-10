import { IHeaderScheme } from "../headers/i-header-scheme.js";

export interface HmacManagerOptions {
    readonly policy: string;
    readonly scheme: IHeaderScheme;
    readonly maxAge: Date; // Use moment.js ?
}