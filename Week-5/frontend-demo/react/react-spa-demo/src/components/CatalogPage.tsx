import { useState } from "react";
import { BookCard } from "./BookCard";
import { catalog } from "../data/catalog";

//

export function CatalogPage(){
    // We are going to use a hook called useState to store information that will persist
    // Between component re-renders as well as be passed down to the child.
    // Hooks are functions that allow you to do certain things based on the component lifecycle
    // Hook rules (this is from the React documentation)
    // 1. Only call on hooks from the top level (no Loops, no nested functions, not inside a try-catch)
    // 2. Only call Hooks from react functions (i.e. function components and custom hooks you write)

    // Let's use that useState hook to stroe some info - today items holds static info
    // from a file. tomorrow from the API.
    const [items] = useState(catalog);

    // Local UI state - here we will hold whether a user has toggled "compact" made on a BookCard
    // and then pass it down as a prop. When the user toggles this value (via a button or something)
    // state is updated, React re-renders, and the cards render "compact"
    const [compact, setCompact] = useState(false);
    
    return(
        <>
        <div className="toolbar">
            <h2>Catalog</h2>
            <button type="button" onClick={()=>setCompact((c)=>!c)}>
                {compact ? "Show detail" : "Compact View"}
            </button>
        </div>
        {/* Here we can render our book cards - we have a list that we don't know the size of because it comes from the API 
        - from outside our frontend app. We will render one BookCard per item in the items array */}
        <div className="cards">
            {
                items.map((item) => (
                    // The minimum to render a bookcard is a item and compact - its props
                    // because we are rendering based on a list - we want to pass a key
                    // so react can track specific BookCard's
                    <BookCard key = {item.sku} item={item} compact={compact}/>
                ))
            }
        </div>
        </>
    )
}