

#include <string.h>
#include <stdio.h>
#include <stdlib.h>
#include <openssl/evp.h>
#include <openssl\ssl.h>

#define AES_BLOCK_SIZE 256

/**
 * Create an 256 bit key and IV using the supplied key_data. salt can be added for taste.
 * Fills in the encryption and decryption ctx objects and returns 0 on success
 **/
int aes_init(unsigned char *key_data, int key_data_len, unsigned char *salt, EVP_CIPHER_CTX *e_ctx,
             EVP_CIPHER_CTX *d_ctx)
{
  int i, nrounds = 5;
  unsigned char key[32], iv[32];

  /*
   * Gen key & IV for AES 256 CBC mode. A SHA1 digest is used to hash the supplied key material.
   * nrounds is the number of times the we hash the material. More rounds are more secure but
   * slower.
   */
  i = EVP_BytesToKey(EVP_aes_256_cbc(), EVP_sha1(), salt, key_data, key_data_len, nrounds, key, iv);
  if (i != 32) {
    printf("Key size is %d bits - should be 256 bits\n", i);
    return -1;
  }

  for(int x = 0; x<32; ++x)
  printf("Key: %x iv: %x \n", key[x], iv[x]);

  for(int x = 0; x<8; ++x)
  printf("salt: %x\n", salt[x]);

  EVP_CIPHER_CTX_init(e_ctx);
  EVP_EncryptInit_ex(e_ctx, EVP_aes_256_cbc(), NULL, key, iv);
  EVP_CIPHER_CTX_init(d_ctx);
  EVP_DecryptInit_ex(d_ctx, EVP_aes_256_cbc(), NULL, key, iv);
  
  return 0;
}

/*
 * Encrypt *len bytes of data
 * All data going in & out is considered binary (unsigned char[])
 */
unsigned char *aes_encrypt(EVP_CIPHER_CTX *e, unsigned char *plaintext, int *len)
{
  /* max ciphertext len for a n bytes of plaintext is n + AES_BLOCK_SIZE -1 bytes */
  int c_len = *len + AES_BLOCK_SIZE - 1, f_len = 0;
  unsigned char *ciphertext = (unsigned char *)malloc(c_len);

  /* allows reusing of 'e' for multiple encryption cycles */
  if(!EVP_EncryptInit_ex(e, NULL, NULL, NULL, NULL)){
    printf("ERROR in EVP_EncryptInit_ex \n");
    return NULL;
  }

  /* update ciphertext, c_len is filled with the length of ciphertext generated,
    *len is the size of plaintext in bytes */
  if(!EVP_EncryptUpdate(e, ciphertext, &c_len, plaintext, *len)){
    printf("ERROR in EVP_EncryptUpdate \n");
    return NULL;
  }

  /* update ciphertext with the final remaining bytes */
  if(!EVP_EncryptFinal_ex(e, ciphertext+c_len, &f_len)){
    printf("ERROR in EVP_EncryptFinal_ex \n");
    return NULL;
  }

  *len = c_len + f_len;
  return ciphertext;
}

/*
 * Decrypt *len bytes of ciphertext
 */
unsigned char *aes_decrypt(EVP_CIPHER_CTX *e, unsigned char *ciphertext, int *len)
{
  /* plaintext will always be equal to or lesser than length of ciphertext*/
  int p_len = *len, f_len = 0;
  unsigned char *plaintext = (unsigned char *)malloc(p_len);

  if(!EVP_DecryptInit_ex(e, NULL, NULL, NULL, NULL)){
    printf("ERROR in EVP_DecryptInit_ex \n");
    return NULL;
  }

  if(!EVP_DecryptUpdate(e, plaintext, &p_len, ciphertext, *len)){
    printf("ERROR in EVP_DecryptUpdate\n");
    return NULL;
  }

  if(!EVP_DecryptFinal_ex(e, plaintext+p_len, &f_len)){
    printf("ERROR in EVP_DecryptFinal_ex\n");
    return NULL;
  }

  *len = p_len + f_len;
  return plaintext;
}

int main(int argc, char **argv)
{
  /* "opaque" encryption, decryption ctx structures that libcrypto uses to record
     status of enc/dec operations */
  EVP_CIPHER_CTX en, de;


  /* The salt paramter is used as a salt in the derivation: it should point to an 8 byte buffer or NULL if no salt is used. */
  unsigned char salt[] = {1,2,3,4,5,6,7,8};
  
  unsigned char *key_data;
  int key_data_len, i;
  char *input[] = {"a", "abcd", "this is a test", "this is a bigger test",
                   "\nWho are fyou ?\nI am the 'Doctor'.\n'Doctor' who ?\nPrecisely!",
                   "üöüäödß",
                   NULL};

  /* the key_data is read from the argument list */
  key_data = (unsigned char *)argv[1];
  key_data_len = strlen(argv[1]);

  /* gen key and iv. init the cipher ctx object */
  if (aes_init(key_data, key_data_len, salt, &en, &de)) {
    printf("Couldn't initialize AES cipher\n");
    return -1;
  }

  /* encrypt and decrypt each input string and compare with the original */
  for (i = 0; input[i]; i++) {
    char *plaintext;
    unsigned char *ciphertext;
    int olen, len;

    /* The enc/dec functions deal with binary data and not C strings. strlen() will
       return length of the string without counting the '\0' string marker. We always
       pass in the marker byte to the encrypt/decrypt functions so that after decryption
       we end up with a legal C string */
    olen = len = strlen(input[i])+1;

    ciphertext = aes_encrypt(&en, (unsigned char *)input[i], &len);
    plaintext = (char *)aes_decrypt(&de, ciphertext, &len);

    if (strncmp(plaintext, input[i], olen))
      printf("FAIL: enc/dec failed for \"%s\"\n", input[i]);
    else
      printf("OK: enc/dec ok for \"%s\"\n", plaintext); // \"%s\"\n

    free(ciphertext);
    free(plaintext);
  }

  EVP_CIPHER_CTX_cleanup(&de);
  EVP_CIPHER_CTX_cleanup(&en);

  return 0;
}
