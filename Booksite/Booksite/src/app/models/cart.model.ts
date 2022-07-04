import { BookDetails } from "./books.model";

export interface Cart {
  items: CartItem[];
}

export interface CartItem {
  isbn: string;
  details?: BookDetails;
  desiredQuantity: number;
}

export interface CartDetailsResponse {
  details: BookDetails[];
}
