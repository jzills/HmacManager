import { IHeaderScheme } from "../headers/i-header-scheme.js";

export class HmacManagerOptions {
    readonly policy: string;
    readonly scheme: IHeaderScheme;
    readonly maxAge: Date; // Use moment.js ?
}