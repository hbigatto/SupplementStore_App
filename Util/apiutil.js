const serverURL = "https://localhost:44367/api/"

//const fetcher = async (endpoint) => {
//let payload;
//try {
//let response = await fetch(`${serverURL}${endpoint}`);
//payload = await response.json();
//} catch (err) {
//console.log(err);
//payload = {error: `Error has occured: ${err.message}`};
//}
//return payload;
//}// end method fetcher

const fetcher = async (endpoint) => {
    let payload;
    let headers = buildHeaders();
    try {
    let response = await fetch(`${serverURL}${endpoint}`, {
    method: "GET",
    headers: headers
    });
    payload = await response.json();
    } catch (err) {
    console.log(err);
    payload = {error: `Error has occured: ${err.message}`};
    }
    return payload;
}// end method fetcher

const poster = async (endpoint, dataToPost) => {
    let payload;
    let headers = buildHeaders();
    try {
    let response = await fetch(`${serverURL}${endpoint}`, {
    method: "POST",
    //headers: { "Content-Type": "application/json; charset=utf-8" },
    headers: headers,
    body: JSON.stringify(dataToPost)
    });
    payload = await response.json();
    } catch (error) {
    payload = error;
    }
    return payload;
}// end method poster

const buildHeaders = () => {
    const myHeaders = new Headers();
    const customer = JSON.parse(sessionStorage.getItem("customer"));
    if (customer) {
    myHeaders.append("Content-Type", "application/json");
    myHeaders.append("Authorization", "Bearer " + customer.token);
    } else {
    myHeaders.append("Content-Type", "application/json");
    }
    return myHeaders;
}//end method buildHeaders

export { fetcher };
export { poster };