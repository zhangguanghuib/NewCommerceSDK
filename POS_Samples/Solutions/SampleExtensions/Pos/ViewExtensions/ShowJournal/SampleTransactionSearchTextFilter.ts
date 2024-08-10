import { ISearchFilterDefinitionContext, CustomTextSearchFilterDefinitionBase } from "PosApi/Extend/Views/CustomSearchFilters";

export default class SampleTransactionSearchTextFilter extends CustomTextSearchFilterDefinitionBase {
    protected readonly labelValue: string;
    protected readonly id: string;

    /**
     * Creates a new instance of the SampleTransactionSearchTextFilter class.
     * @param {ISearchFilterDefinitionContext} context The search filter definition context.
     */
    constructor(context: ISearchFilterDefinitionContext) {
        super(context);

        this.id = "SampleTransactionSearchTextFilter";
        this.labelValue = "Text Search Filter Label";

        
    }
}