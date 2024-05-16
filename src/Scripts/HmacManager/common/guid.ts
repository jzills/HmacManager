export class Guid {
    private static readonly pattern: RegExp = /^([0-9A-Fa-f]{8}-([0-9A-Fa-f]{4}-){3}[0-9A-Fa-f]{12})$/g;
    private static readonly regExp: RegExp = new RegExp(Guid.pattern);

    constructor(value: string) {
        const result = value.match(Guid.regExp);
        if (result && result.length) {
            this.value = value;
        } else {
            this.value = Guid.empty;
        }
    }

    public static readonly empty: string = `${"0".repeat(8)}-${"0".repeat(4).concat("-").repeat(3)}${"0".repeat(12)}`
    public readonly value: string;
}