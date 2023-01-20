<template>
    <div class="container">
        <img class="item-image" style="margin-top: 2vh" src="../assets/cart.png"/>
    <div class="heading">Cart contents</div>
    <div class="status">{{ state.status }}</div>

    <div v-if="state.cart.length > 0" id="cart">
        <DataTable
        :value="state.cart"
        :scrollable="true"
        scrollHeight="38vh"
        dataKey="id"
        class="p-datatable-striped"
        id="cart-contents"
        >
            

            <Column id="desc" header="Product" field="item.productName" />
            <Column header="Qty" field="qty" style="margin-right: 0vw" />

            <Column header="Price" field="item.costPrice" >
                 <template #body="slotProps">
                    {{ formatCurrency(slotProps.data.item.costPrice) }}
                </template>
            </Column>

            <Column header="Extended" field="item.costPrice" >
               <template #body="slotProps">
                    {{ formatCurrency(slotProps.data.item.costPrice * slotProps.data.qty) }}
                </template>
            </Column>
            
     
        </DataTable>
       
       
    </div>

    <div v-if="state.cart.length > 0">
        <div class="cart-head">Cart Totals</div>

        <table style="border: solid; margin-top: 1vh">
        <tr>
            <td style="width: 20%; font-weight: bold">Sub:</td>
            <td style="width: 10%; text-align: right; padding-right: 3vw">{{ formatCurrency(state.subTotal) }}</td>
      
        </tr>
        <tr>
            <td style="width: 20%; font-weight: bold">Tax:</td>
            <td style="width: 10%; text-align: right; padding-right: 3vw">{{formatCurrency(state.taxTotal) }}</td>
      
        </tr>
         <tr>
            <td style="width: 20%; font-weight: bold">Total:</td>
            <td style="width: 10%; text-align: right; padding-right: 3vw">{{ formatCurrency(state.totalPrice) }}</td>
      
        </tr>
        </table>
    </div>

    <Button style="margin-top: 2vh" label="Add Order" @click="saveCart" class="p-button-outlined margin-button1" />
    &nbsp;
    <Button style="margin-top: 2vh" label="Clear Cart" @click="clearCart" class="p-button-outlined margin-button1"/>

    </div>

    

</template>

<script>

import { reactive, onMounted } from "vue";
import { poster } from "../util/apiutil"; 

export default {
setup() {
onMounted(() => {
});

let state = reactive({
status: "",
subTotal:0,
taxTotal:0,
tax:0.13,
totalPrice:0,
cart: [],
}); //end reactive

onMounted(() => {
if (sessionStorage.getItem("cart")) {
state.cart = JSON.parse(sessionStorage.getItem("cart"));
state.cart.map((cartItem) => {

state.subTotal += cartItem.item.costPrice * cartItem.qty;
state.taxTotal += (cartItem.item.costPrice * cartItem.qty) * state.tax;
state.totalPrice = state.subTotal + state.taxTotal;

});
} else {
state.cart = [];
}
}); //end onMounted method

const formatCurrency = (value) => {
return value.toLocaleString("en-US", {
style: "currency",
currency: "USD",
});
}; //end method formatCurrency

const clearCart = () => {
sessionStorage.removeItem("cart");
state.cart = [];
state.status = "cart cleared";
};//end Clear Cart

const saveCart = async () => {
let customer = JSON.parse(sessionStorage.getItem("customer"));
let cart = JSON.parse(sessionStorage.getItem("cart"));
try {
state.status = "sending cart info to server";
let orderHelper = { email: customer.email, selections: cart };

let payload = await poster("order", orderHelper);
if (payload.indexOf("not") > 0) {
state.status = payload;
} else {
clearCart();
state.status = payload;
}
} catch (err) {
console.log(err);
state.status = `Error add cart: ${err}`;
}
}; //end method saveCart


return {
state,
formatCurrency,
clearCart,
saveCart,

};

} //end default
} //end setup

</script>

<style>
#cart-contents th:nth-child(2) {
margin-left: 0vw;
}
</style>

