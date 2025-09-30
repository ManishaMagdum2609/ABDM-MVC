import { AbhaLoginApiService } from "../../../../Infrastructure/Services/abhaLogin-api.service";
import { SearchAbhaAccountQuery } from "../search-abha-account.query";



export class SearchAbhaAccountHandler {
  constructor(private apiService: AbhaLoginApiService) {}

  async execute(query: SearchAbhaAccountQuery) {
    return this.apiService.SearchAbha(query.mobile);
  }
}