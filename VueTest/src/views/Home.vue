<template>
  <div class="home">
    <img alt="Vue logo" src="../assets/logo.png">
    <br><br>
    This is the home page.
    <router-link :to="{ name: 'about' }">Click here to see our about page</router-link>
    <br><br><br><hr><br>
    <HelloWorld msg="Welcome to Your Vue.js App"/>
  </div>
</template>

<script>
// @ is an alias to /src
import HelloWorld from '@/components/HelloWorld.vue'

export default {
  name: 'home',
  components: {
    HelloWorld
  }
}
</script>
