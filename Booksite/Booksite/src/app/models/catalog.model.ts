export interface Book {
  isbn: string;
  title: string;
  author: string;
  quantity: number;
  price: number;
  coverFile: string;
}
  
export interface CatalogPage {
  books: Book[],
  count: number
}