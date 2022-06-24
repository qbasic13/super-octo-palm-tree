import { Component } from '@angular/core';
import { MatDialog, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ActivatedRoute } from '@angular/router';
import { catchError, map, of } from 'rxjs';
import { BookDetails } from 'src/app/models/books.model';
import { EditBookComponent } from '../edit-book/edit-book.component';
import { AuthService } from '../services/auth.service';
import { BookService } from '../services/book.service';

@Component({
  selector: 'app-book-details',
  templateUrl: './book-details.component.html',
  styleUrls: ['./book-details.component.css']
})
export class BookDetailsComponent {
  book: any;
  isError: boolean = false;
  hide: boolean = true;
  errorText: string = "";
  constructor(
    private authService: AuthService,
    private bookService: BookService,
    private route: ActivatedRoute,
    public editDialog: MatDialog) {

    const isbn = this.route.params.subscribe(params => {
      this.hide = true;
      this.isError = false;
      this.fetchBookData(params['isbn']);
    });
  }

  fetchBookData(isbn: string) {
    if (BookService.isValidIsbn(isbn)) {
      this.bookService.getBookDetails(isbn).subscribe(
        (result) => {
          this.book = result as BookDetails;
          this.hide = false;
        },
        (error) => {
          this.errorText = error.message;
          this.isError = true;
          this.hide = false;
          return of(null);
        }
      );
    } else {
      this.errorText = "incorrect ISBN";
      this.isError = true;
      this.hide = false;
    }
  }

  hasRole(...params: string[]) {
    return this.authService.hasRole(...params);
  }

  openEditDialog() {
    this.editDialog.open(EditBookComponent, {
      data: {
        book: this.book
      },
      height: '80vh'
    });
  }

  isLoggedIn() {
    return this.authService.isLoggedIn();
  }
}
