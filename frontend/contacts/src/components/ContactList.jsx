import { useState, useEffect } from 'react';

const ContactList = () => {
  const [isLoading, setIsLoading] = useState(true);
  const [contacts, setContacts] = useState([]);

  const fetchContacts = async () => {
    const response = await fetch('http://localhost:5000/contacts');
    const data = await response.json();
    return data;
  };

  useEffect(() => {
    const getContacts = async () => {
      const contactsFromServer = await fetchContacts();
      setContacts((prev) => [...prev, contactsFromServer]);
      setIsLoading(false);
    };
    getContacts();
  }, []);

  return isLoading ? (
    <div>Loading...</div>
  ) : (
    <div>
      {contacts.map((contact) => (
        <div key={contact.id}>
          <h3>{contact.name}</h3>
          <p>{contact.email}</p>
        </div>
      ))}
    </div>
  );
};

export default ContactList;
